# could use like {{ "style.css" | hash_assets }} and return style.css?v=abc123... but it does not work in github
module Jekyll
    module CacheBust
      class CacheDigester
        require 'digest/md5'

        attr_accessor :file_name, :directory

        def initialize(file_name:, directory: nil)
          self.file_name = file_name
          self.directory = directory
        end

        def digest!
          [file_name, '?v=', Digest::MD5.hexdigest(file_contents)].join
        end

        private

        def directory_files_content
          target_path = File.join(directory, '**', '*')
          Dir[target_path].map{|f| File.read(f) unless File.directory?(f) }.join
        end

        def file_content
          FIle.read(file_name)
        end

        def file_contents
          is_directory? ? file_content : directory_files_content
        end

        def is_directory?
          directory.nil?
        end
      end

      def hash_assets(file_name)
        CacheDigester.new(file_name: file_name, directory: 'assets').digest!
      end
    end
  end

  Liquid::Template.register_filter(Jekyll::CacheBust)
